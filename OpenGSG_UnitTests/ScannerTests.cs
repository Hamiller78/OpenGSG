using OpenGSGLibrary.GameFilesParser;

namespace OpenGSG_UnitTests
{
    [TestFixture]
    public class ScannerTests
    {
        [Test]
        public void Test_Scan_StringReader()
        {
            var testScanner = new Scanner();
            var inputText = new StringReader(
                "state={ \n       id=92     # comment to be ignored\n       name=\"Ülék?o\"\n      }"
            );
            var outputStream = testScanner.Scan(inputText);

            outputStream.MoveNext();
            Assert.That(outputStream.Current.kind, Is.EqualTo(Kind.KEYWORD));
            Assert.That(outputStream.Current.ToString(), Is.EqualTo("state"));

            outputStream.MoveNext();
            Assert.That(outputStream.Current.kind, Is.EqualTo(Kind.EQUAL));

            outputStream.MoveNext();
            Assert.That(outputStream.Current.kind, Is.EqualTo(Kind.LEFTBRACKET));

            outputStream.MoveNext();
            Assert.That(outputStream.Current.kind, Is.EqualTo(Kind.KEYWORD));
            Assert.That(outputStream.Current.ToString(), Is.EqualTo("id"));

            outputStream.MoveNext();
            Assert.That(outputStream.Current.kind, Is.EqualTo(Kind.EQUAL));

            outputStream.MoveNext();
            Assert.That(outputStream.Current.kind, Is.EqualTo(Kind.NUMBER));
            Assert.That(outputStream.Current.ToString(), Is.EqualTo("NUMBER(92)"));

            outputStream.MoveNext();
            Assert.That(outputStream.Current.kind, Is.EqualTo(Kind.KEYWORD));
            Assert.That(outputStream.Current.ToString(), Is.EqualTo("name"));

            outputStream.MoveNext();
            Assert.That(outputStream.Current.kind, Is.EqualTo(Kind.EQUAL));

            outputStream.MoveNext();
            Assert.That(outputStream.Current.kind, Is.EqualTo(Kind.KEYWORD));
            Assert.That(outputStream.Current.ToString(), Is.EqualTo("Ülék?o"));

            outputStream.MoveNext();
            Assert.That(outputStream.Current.kind, Is.EqualTo(Kind.RIGHTBRACKET));

            outputStream.MoveNext();
            Assert.That(outputStream.Current.kind, Is.EqualTo(Kind.EOF));
        }
    }
}
