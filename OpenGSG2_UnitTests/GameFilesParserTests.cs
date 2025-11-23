using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Parser;

namespace OpenGSG2_UnitTests
{
    [TestFixture]
    public class GameFilesParserTests
    {
        [Test]
        public void Test_Parse()
        {
            var testParser = new Parser.Parser();

            var testResult = testParser.Parse(InputStream().GetEnumerator());

            var stateProps = (ILookup<string, object>)testResult["state"].First();

            var id = Convert.ToInt32(stateProps["id"].First());
            Assert.That(id, Is.EqualTo(92));

            var name = stateProps["name"].First().ToString();
            Assert.That(name, Is.EqualTo("STATE_92"));

            var provinceList = (List<int>)stateProps["provinces"].First();
            Assert.That(provinceList[0], Is.EqualTo(13));
            Assert.That(provinceList[1], Is.EqualTo(13423));
            Assert.That(provinceList[2], Is.EqualTo(908));
        }

        [Test]
        public void Test_ParseEmptyCollection()
        {
            var testParser = new Parser.Parser();
            var testResult = testParser.Parse(InputStreamWithEmptyCollection().GetEnumerator());

            var countryProps = (ILookup<string, object>)testResult["country"].First();

            var tag = countryProps["tag"].First().ToString();
            Assert.That(tag, Is.EqualTo("FRG"));

            var name = countryProps["name"].First().ToString();
            Assert.That(name, Is.EqualTo("Germany"));

            Assert.That(countryProps.Contains("resources"), Is.False);
        }

        [Test]
        public void Test_ParseDuplicateKeyCollection()
        {
            var testParser = new Parser.Parser();
            var testResult = testParser.Parse(InputStreamWithDuplicateKey().GetEnumerator());

            var armyProps = (ILookup<string, object>)testResult["army"].First();

            Assert.That(armyProps["name"].Count(), Is.EqualTo(1));
            var name = armyProps["name"].First().ToString();
            Assert.That(name, Is.EqualTo("Some army name"));

            var location = Convert.ToInt32(armyProps["location"].First());
            Assert.That(location, Is.EqualTo(4));

            Assert.That(armyProps["division"].Count(), Is.EqualTo(2));

            var division0 = (ILookup<string, object>)armyProps["division"].ElementAt(0);
            Assert.That(division0["name"].First().ToString(), Is.EqualTo("Division 1"));
            Assert.That(division0["type"].First().ToString(), Is.EqualTo("Infantry"));
            Assert.That(Convert.ToInt32(division0["size"].First()), Is.EqualTo(10000));

            var division1 = (ILookup<string, object>)armyProps["division"].ElementAt(1);
            Assert.That(division1["name"].First().ToString(), Is.EqualTo("Division 2"));
            Assert.That(division1["type"].First().ToString(), Is.EqualTo("Armor"));
            Assert.That(Convert.ToInt32(division1["size"].First()), Is.EqualTo(300));
        }

        private IEnumerable<Token> InputStream()
        {
            yield return Token.FromString("state");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromKind(Kind.LEFTBRACKET);

            yield return Token.FromString("id");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromValue(92);

            yield return Token.FromString("name");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromString("STATE_92");

            yield return Token.FromString("provinces");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromKind(Kind.LEFTBRACKET);
            yield return Token.FromValue(13);
            yield return Token.FromValue(13423);
            yield return Token.FromValue(908);
            yield return Token.FromKind(Kind.RIGHTBRACKET);

            yield return Token.FromKind(Kind.RIGHTBRACKET);
            yield return Token.FromKind(Kind.EOF);
        }

        private IEnumerable<Token> InputStreamWithEmptyCollection()
        {
            yield return Token.FromString("country");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromKind(Kind.LEFTBRACKET);

            yield return Token.FromString("tag");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromString("FRG");

            yield return Token.FromString("name");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromString("Germany");

            yield return Token.FromString("resources");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromKind(Kind.LEFTBRACKET);
            yield return Token.FromKind(Kind.RIGHTBRACKET);

            yield return Token.FromKind(Kind.RIGHTBRACKET);
            yield return Token.FromKind(Kind.EOF);
        }

        private IEnumerable<Token> InputStreamWithDuplicateKey()
        {
            yield return Token.FromString("army");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromKind(Kind.LEFTBRACKET);

            yield return Token.FromString("name");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromString("Some army name");

            yield return Token.FromString("location");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromValue(4);

            yield return Token.FromString("division");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromKind(Kind.LEFTBRACKET);

            yield return Token.FromString("name");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromString("Division 1");

            yield return Token.FromString("type");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromString("Infantry");

            yield return Token.FromString("size");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromValue(10000);

            yield return Token.FromKind(Kind.RIGHTBRACKET);
            yield return Token.FromString("division");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromKind(Kind.LEFTBRACKET);

            yield return Token.FromString("name");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromString("Division 2");

            yield return Token.FromString("type");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromString("Armor");

            yield return Token.FromString("size");
            yield return Token.FromKind(Kind.EQUAL);
            yield return Token.FromValue(300);

            yield return Token.FromKind(Kind.RIGHTBRACKET);

            yield return Token.FromKind(Kind.RIGHTBRACKET);
            yield return Token.FromKind(Kind.EOF);
        }
    }
}
