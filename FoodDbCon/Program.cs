// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FoodDbCon
{
	internal class Program
	{
		private static List<string> GetFoodIds()
		{
			var url = "http://www.bedca.net/bdpub/procquery.php";
			var web = new WebClient();
			web.Encoding = Encoding.UTF8;
			var resp = web.UploadString(url,
				@"<?xml version=""1.0"" encoding=""utf-8""?>
<foodquery>
<type level=""1""/>
<selection>
	<atribute name=""f_id""/>
</selection>
</foodquery>
");
			var xml = XDocument.Parse(resp);
			var food = from c in xml.Descendants("food")
				from d in c.Descendants("f_id")
				select d.Value;
			return food.ToList();
		}

		private static void Main(string[] args)
		{
			Console.WriteLine("Querying food ids");
			var r = GetFoodIds();
			Console.WriteLine("Found {0} food records", r.Count);
			var cbag = new ConcurrentBag<Food>();
			var option = new ParallelOptions();
			option.MaxDegreeOfParallelism = 60;
			var count = 0;
			var dictionary = new ConcurrentDictionary<int, FoodAttributeDescriptor>();
			var maxRetries = 10;
			Parallel.ForEach(r, option, id =>
			{
				Interlocked.Increment(ref count);
				var retry = 0;
				while (retry++ < maxRetries)
				{
					try
					{
						var info = GetFoodInfo(id, dictionary);
						cbag.Add(info);
						Console.WriteLine("[{0}/{1}] Retrieved Id:{2} Name:{3}", count, r.Count, id, info.Name);
						return;
					}
					catch (WebException)
					{
						Console.WriteLine("[{0}/{1}] Id without response: {2}", count, r.Count, id);
					}
				}
			});
			var foodDb = new FoodDataSet();
			foodDb.FetchDate = DateTime.Now;
			foodDb.Foods = cbag.ToList();
			foodDb.Descriptors = dictionary.Values.ToList();
			var ser = new DataContractSerializer(typeof (FoodDataSet));
			var file = File.OpenWrite("foodDb.xml");
			ser.WriteObject(file, foodDb);
			file.Close();
		}

		private static FoodAttributeDescriptor GetDescriptor(IDictionary<int, FoodAttributeDescriptor> descriptors, int id,
			XElement element)
		{
			FoodAttributeDescriptor val;
			if (descriptors.TryGetValue(id, out val))
			{
				return val;
			}

			var name = element.Element("c_ori_name")?.Value;
			var eur_name = element.Element("eur_name")?.Value;

			val = new FoodAttributeDescriptor();
			val.Id = id;
			val.Name = name;
			val.EurName = eur_name;
			return val;
		}

		private static Food GetFoodInfo(string id, IDictionary<int, FoodAttributeDescriptor> descriptors)
		{
			var url = "http://www.bedca.net/bdpub/procquery.php";
			var web = new WebClient();
			web.Encoding = Encoding.UTF8;
			var resp = web.UploadString(url,
				$@"<?xml version=""1.0"" encoding=""utf-8""?>
<foodquery>
<type level=""2""/>
<selection>
	<atribute name=""f_id""/>
	<atribute name=""f_ori_name""/>
	<atribute name=""mainlevelcode""/>
	<atribute name=""namelevel1""/>
	<atribute name=""namelevel2""/>
	<atribute name=""edible_portion""/>
	<atribute name=""c_id""/>
	<atribute name=""c_ori_name""/>
	<atribute name=""eur_name""/>
	<atribute name=""v_unit""/>
	<atribute name=""best_location""/>	
</selection>
<condition>
	<cond1>
		<atribute1 name=""f_id""/>	
	</cond1>
	<relation type=""EQUAL""/>
	<cond3>{
					id
					}</cond3>
</condition>
<order ordtype=""ASC"">
	<atribute3 name=""componentgroup_id""/>
</order>
</foodquery>");

			var xml = XDocument.Parse(resp);
			var foodItem = xml.Descendants("food").First();

			var foodBlock = new Food();
			foodBlock.Id = int.Parse(id);
			foodBlock.Name = foodItem.Element("f_ori_name")?.Value;
			foodBlock.EnglishName = foodItem.Element("f_eng_name")?.Value;
			foodBlock.Category1 = foodItem.Element("mainlevelcode")?.Value;
			foodBlock.Category2 = foodItem.Element("namelevel1")?.Value;
			foodBlock.Category3 = foodItem.Element("namelevel2")?.Value;
			foodBlock.Attributes = new List<FoodAttribute>();

			var atts = from c in foodItem.Descendants("foodvalue") select c;
			foreach (var att in atts)
			{
				var attid = int.Parse(att.Element("c_id")?.Value);
				var valStr = att.Element("best_location")?.Value;
				var val = !string.IsNullOrWhiteSpace(valStr) ? (float?) float.Parse(valStr) : null;
				var item = new FoodAttribute
				{
					Id = attid,
					Unit = att.Element("v_unit")?.Value,
					Descriptor = GetDescriptor(descriptors, attid, att),
					Value = val
				};

				foodBlock.Attributes.Add(item);
			}

			return foodBlock;
		}
	}

	[DataContract]
	public class FoodDataSet
	{
		[DataMember]
		public List<FoodAttributeDescriptor> Descriptors { get; set; }

		[DataMember]
		public List<Food> Foods { get; set; }

		[DataMember]
		public DateTime FetchDate { get; set; }
	}

	[DataContract(IsReference = true)]
	public class FoodAttributeDescriptor
	{
		[DataMember(Name = "i")]
		public int Id { get; set; }

		[DataMember(Name = "n")]
		public string Name { get; set; }

		[DataMember(Name = "tn")]
		public string EurName { get; set; }
	}

	[DataContract(IsReference = true)]
	public class Food
	{
		[DataMember(Name = "i")]
		public int Id { get; set; }

		[DataMember(Name = "n")]
		public string Name { get; set; }

		[DataMember(Name = "en")]
		public string EnglishName { get; set; }

		[DataMember(Name = "c1")]
		public string Category1 { get; set; }

		[DataMember(Name = "c2")]
		public string Category2 { get; set; }

		[DataMember(Name = "c3")]
		public string Category3 { get; set; }

		[DataMember]
		public List<FoodAttribute> Attributes { get; set; }
	}

	[DataContract(IsReference = true, Name = "a")]
	public class FoodAttribute
	{
		[DataMember(Name = "i")]
		public int Id { get; set; }

		[DataMember(Name = "d")]
		public FoodAttributeDescriptor Descriptor { get; set; }

		[DataMember(Name = "u")]
		public string Unit { get; set; }

		[DataMember(Name = "v")]
		public float? Value { get; set; }

		public override string ToString()
		{
			return $"{Id} - {Value} {Unit} - {Descriptor?.Name}";
		}
	}
}