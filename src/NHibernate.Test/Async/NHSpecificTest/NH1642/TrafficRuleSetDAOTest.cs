﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1642
{
	using System.Threading.Tasks;
	[TestFixture]
	public class TrafficRuleSetDAOTestAsync : BugTestCase
	{
		private class Scenario : IDisposable
		{
			private readonly ISessionFactory sessionFactory;
			private int ruleSetId;
			private string ruleSetName;

			public Scenario(ISessionFactory sessionFactory)
			{
				this.sessionFactory = sessionFactory;
				ruleSetId = 2;
				ruleSetName = "RuleSet" + ruleSetId.ToString();

				using (var session = sessionFactory.OpenSession())
				{
					using (var tr = session.BeginTransaction())
					{
						TrafficRuleSet ruleSet = new TrafficRuleSet {name = ruleSetName, description = ruleSetName};
						TrafficRule rule = new TrafficRule {ruleSet = ruleSet, name = ruleSetName + "-a", description = "Some description"};

						ruleSet.rules = new List<TrafficRule> {rule};

						ruleSetId = (int) session.Save(ruleSet);
						tr.Commit();
					}
				}
			}

			public int RuleSetId
			{
				get { return ruleSetId; }
			}

			public string RuleSetName
			{
				get { return ruleSetName; }
			}

			public void Dispose()
			{
				using (var session = sessionFactory.OpenSession())
				using (var tr = session.BeginTransaction())
				{
					session.Delete("from System.Object");
					tr.Commit();
				}
			}
		}

		[Test]
		public async Task AddRuleSetAsync()
		{
			using (var scenario = new Scenario(Sfi))
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var fromDb = await (session.GetAsync<TrafficRuleSet>(scenario.RuleSetId));
				Assert.IsNotNull(fromDb);
				Assert.AreEqual(fromDb.name, scenario.RuleSetName);
				Assert.AreEqual(fromDb.rules[0].name, scenario.RuleSetName + "-a");
			}
		}
	}
}