using System.Collections.Generic;
using NUnit.Framework;
using Omu.ValueInjecter;

namespace Tests
{
    [TestFixture]
    public class TunnelierTests
    {
        public class Foo
        {
            public string Name { get; set; }
            public Foo Parent { get; set; }
        }

        [Test]
        public void GetValueFromAUnfullBranchReturnNull()
        {
            var o = new Foo() { Parent = new Foo() { } };
            Tunnelier.GetPropertyWithComponent(new Queue<string>(new List<string> { "Parent", "Parent", "Name" }), o).IsEqualTo(null);
        }

        [Test]
        public void GetValueReturns()
        {
            var o = new Foo { Parent = new Foo { Parent = new Foo { Name = "hey" } } };
            var endpoint = Tunnelier.GetPropertyWithComponent(new Queue<string>(new List<string> { "Parent", "Parent", "Name" }), o);
#if !WindowsCE
            endpoint.Property.GetValue(endpoint.Component).IsEqualTo("hey");
#else
            endpoint.Property.GetValue(endpoint.Component, null).IsEqualTo("hey");
#endif
        }

        [Test]
        public void DiggDuggs()
        {
            var o = new Foo();
            Tunnelier.Digg(new Queue<string>(new List<string> {"Parent", "Parent", "Name"}), o).IsNotNull();
        }

    }
}