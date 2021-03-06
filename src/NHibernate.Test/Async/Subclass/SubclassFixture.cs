﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.Subclass
{
	using System.Threading.Tasks;
	/// <summary>
	/// Test the use of <c>&lt;class&gt;</c> and <c>&lt;subclass&gt;</c> mappings.
	/// </summary>
	[TestFixture]
	public class SubclassFixtureAsync : TestCase
	{

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new string[] {"Subclass.Subclass.hbm.xml"}; }
		}

		[Test]
		public async Task TestCRUDAsync()
		{
			// test the Save
			ISession s1 = OpenSession();
			ITransaction t1 = s1.BeginTransaction();
			int oneId;
			int baseId;

			SubclassOne one1 = new SubclassOne();

			one1.TestDateTime = new DateTime(2003, 10, 17);
			one1.TestString = "the test one string";
			one1.TestLong = 6;
			one1.OneTestLong = 1;

			oneId = (int) await (s1.SaveAsync(one1));

			SubclassBase base1 = new SubclassBase();
			base1.TestDateTime = new DateTime(2003, 10, 17);
			base1.TestString = "the test string";
			base1.TestLong = 5;

			baseId = (int) await (s1.SaveAsync(base1));

			await (t1.CommitAsync());
			s1.Close();

			// lets verify the correct classes were saved
			ISession s2 = OpenSession();
			ITransaction t2 = s2.BeginTransaction();

			// perform a load based on the base class
			SubclassBase base2 = (SubclassBase) await (s2.LoadAsync(typeof(SubclassBase), baseId));
			SubclassBase oneBase2 = (SubclassBase) await (s2.LoadAsync(typeof(SubclassBase), oneId));

			// do some quick checks to make sure s2 loaded an object with the same data as s2 saved.
			SubclassAssert.AreEqual(base1, base2);

			// the object with id=2 was loaded using the base class - lets make sure it actually loaded
			// the subclass
			SubclassOne one2 = oneBase2 as SubclassOne;
			Assert.IsNotNull(one2);

			// lets update the objects
			base2.TestString = "Did it get updated";

			// update the properties from the subclass and base class
			one2.TestString = "Updated SubclassOne String";
			one2.OneTestLong = 21;

			// save it through the base class reference and make sure that the
			// subclass properties get updated.
			await (s2.UpdateAsync(base2));
			await (s2.UpdateAsync(oneBase2));

			await (t2.CommitAsync());
			s2.Close();

			// lets test the Criteria interface for subclassing
			ISession s3 = OpenSession();
			ITransaction t3 = s3.BeginTransaction();

			IList results3 = await (s3.CreateCriteria(typeof(SubclassBase))
				.Add(Expression.In("TestString", new string[] {"Did it get updated", "Updated SubclassOne String"}))
				.ListAsync());

			Assert.AreEqual(2, results3.Count);

			SubclassBase base3 = null;
			SubclassOne one3 = null;

			foreach (SubclassBase obj in results3)
			{
				if (obj is SubclassOne)
					one3 = (SubclassOne) obj;
				else
					base3 = obj;
			}

			// verify the properties got updated
			SubclassAssert.AreEqual(base2, base3);
			SubclassAssert.AreEqual(one2, one3);

			await (s3.DeleteAsync(base3));
			await (s3.DeleteAsync(one3));

			await (t3.CommitAsync());
			s3.Close();
		}

		[Test]
		public async Task HqlClassKeywordAsync()
		{
			// test the Save
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			int oneId;
			int baseId;

			SubclassOne one1 = new SubclassOne();

			one1.TestDateTime = new DateTime(2003, 10, 17);
			one1.TestString = "the test one string";
			one1.TestLong = 6;
			one1.OneTestLong = 1;

			oneId = (int) await (s.SaveAsync(one1));

			SubclassBase base1 = new SubclassBase();
			base1.TestDateTime = new DateTime(2003, 10, 17);
			base1.TestString = "the test string";
			base1.TestLong = 5;

			baseId = (int) await (s.SaveAsync(base1));

			await (t.CommitAsync());
			s.Close();

			s = OpenSession();
			IList list = await (s.CreateQuery("from SubclassBase as sb where sb.class=SubclassBase").ListAsync());
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual(typeof(SubclassBase), list[0].GetType(), "should be base");

			list = await (s.CreateQuery("from SubclassBase as sb where sb.class=SubclassOne").ListAsync());
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual(typeof(SubclassOne), list[0].GetType(), "should be one");
			s.Close();

			s = OpenSession();
			await (s.DeleteAsync(one1));
			await (s.DeleteAsync(base1));
			await (s.FlushAsync());
			s.Close();
		}
	}
}
