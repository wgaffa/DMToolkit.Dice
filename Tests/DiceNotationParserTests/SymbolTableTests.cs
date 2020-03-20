using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wgaffa.DMToolkit.Parser;
using Wgaffa.Functional;

namespace DiceNotationParserTests
{
    public class SymbolTableTests
    {
        [Test]
        public void Add_ShouldAddItem_WhenNoDuplicateExist()
        {
            var table = new ScopedSymbolTable();
            table.Add(new BuiltinTypeSymbol("int"));

            Assert.That(table.Depth, Is.EqualTo(1));
        }

        [Test]
        public void Add_ShouldThrow_GivenDuplicateKeys()
        {
            var table = new ScopedSymbolTable();
            table.Add(new BuiltinTypeSymbol("int"));

            Assert.That(() => table.Add(new BuiltinTypeSymbol("int")), Throws.TypeOf<ArgumentException>());
        }

        public IEnumerable<ISymbol> SetupFooBar()
        {
            var real = new BuiltinTypeSymbol("real");
            var integer = new BuiltinTypeSymbol("int");

            yield return real;
            yield return integer;
            yield return new VariableSymbol("foo", integer);
            yield return new VariableSymbol("bar", real);
        }

        [Test]
        public void Lookup_ShouldReturnSome_GivenGivenExistingName()
        {
            var table = new ScopedSymbolTable(SetupFooBar());
            var value = table.Lookup("foo").Reduce(default(ISymbol));

            var expected = new VariableSymbol("foo", new BuiltinTypeSymbol("int"));

            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void Lookup_ShouldReturnNone_GivenNoneExistingName()
        {
            var table = new ScopedSymbolTable(SetupFooBar());
            var value = table.Lookup("fizz");

            Assert.That(value, Is.TypeOf<None<ISymbol>>());
        }

        [TestCase("foo")]
        [TestCase("bar")]
        public void Lookup_ShouldSucceed_GivenNestedScopes(string identifier)
        {
            var foo = new VariableSymbol("foo", new BuiltinTypeSymbol("int"));
            var bar = new VariableSymbol("bar", new BuiltinTypeSymbol("real"));
            var global = new ScopedSymbolTable(new ISymbol[] { foo });
            var nested = new ScopedSymbolTable(new ISymbol[] { bar }, global, 2);

            var result = nested.Lookup(identifier).Reduce(default(ISymbol));

            var expected = identifier == "foo" ? foo : bar;

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
