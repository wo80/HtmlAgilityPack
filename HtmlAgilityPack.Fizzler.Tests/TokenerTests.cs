namespace HtmlAgilityPack.Fizzler.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class TokenizerTests
    {
        [Test]
        public void NullInput()
        {
            Assert.AreEqual(Token.Eoi(), Tokenizer.Tokenize((string) null).Single());
        }

        [Test]
        public void EmptyInput()
        {
            Assert.AreEqual(Token.Eoi(), Tokenizer.Tokenize(string.Empty).Single());
        }

        [Test]
        public void WhiteSpace()
        {
            Assert.AreEqual(Token.WhiteSpace(" \r \n \f \t "), Tokenizer.Tokenize(" \r \n \f \t etc").First());
        }

        [Test]
        public void Colon()
        {
            Assert.AreEqual(Token.Colon(), Tokenizer.Tokenize(":").First());
        }

        [Test]
        public void Comma()
        {
            Assert.AreEqual(Token.Comma(), Tokenizer.Tokenize(",").First());
        }

        [Test]
        public void CommaWhiteSpacePrepended()
        {
            Assert.AreEqual(Token.Comma(), Tokenizer.Tokenize("  ,").First());
        }

        [Test]
        public void Plus()
        {
            Assert.AreEqual(Token.Plus(), Tokenizer.Tokenize("+").First());
        }

        [Test]
        public void Equals()
        {
            Assert.AreEqual(Token.Equals(), Tokenizer.Tokenize("=").First());
        }

        [Test]
        public void LeftBracket()
        {
            Assert.AreEqual(Token.LeftBracket(), Tokenizer.Tokenize("[").First());
        }

        [Test]
        public void RightBracket()
        {
            Assert.AreEqual(Token.RightBracket(), Tokenizer.Tokenize("]").First());
        }

        [Test]
        public void PlusWhiteSpacePrepended()
        {
            Assert.AreEqual(Token.Plus(), Tokenizer.Tokenize("  +").First());
        }

        [Test]
        public void RightParenthesis()
        {
            Assert.AreEqual(Token.RightParenthesis(), Tokenizer.Tokenize(")").First());
        }

        [Test]
        public void Greater()
        {
            Assert.AreEqual(Token.Greater(), Tokenizer.Tokenize(">").First());
        }

        [Test]
        public void GreaterWhiteSpacePrepended()
        {
            Assert.AreEqual(Token.Greater(), Tokenizer.Tokenize("  >").First());
        }

        [Test]
        public void IdentifierLowerCaseOnly()
        {
            Assert.AreEqual(Token.Ident("foo"), Tokenizer.Tokenize("foo").First());
        }

        [Test]
        public void IdentifierMixedCase()
        {
            Assert.AreEqual(Token.Ident("FoObAr"), Tokenizer.Tokenize("FoObAr").First());
        }

        [Test]
        public void IdentifierIncludingDigits()
        {
            Assert.AreEqual(Token.Ident("foobar42"), Tokenizer.Tokenize("foobar42").First());
        }

        [Test]
        public void IdentifierWithUnderscores()
        {
            Assert.AreEqual(Token.Ident("_foo_BAR_42_"), Tokenizer.Tokenize("_foo_BAR_42_").First());
        }

        [Test]
        public void IdentifierWithHypens()
        {
            Assert.AreEqual(Token.Ident("foo-BAR-42"), Tokenizer.Tokenize("foo-BAR-42").First());
        }

        [Test]
        public void IdentifierUsingVendorExtensionSyntax()
        {
            Assert.AreEqual(Token.Ident("-foo-BAR-42"), Tokenizer.Tokenize("-foo-BAR-42").First());
        }

        [Test]
        public void IdentifierUsingVendorExtensionSyntaxCannotBeginWithDigit()
        {
            Assert.Throws<FormatException>(() => Tokenizer.Tokenize("-42").ToArray());
        }

        [Test]
        public void Hash()
        {
            Assert.AreEqual(Token.Hash("foo_BAR-baz-42"), Tokenizer.Tokenize("#foo_BAR-baz-42").First());
        }

        [Test]
        public void Includes()
        {
            Assert.AreEqual(TokenKind.Includes, Tokenizer.Tokenize("~=").First().Kind);
        }

        [Test]
        public void RegexMatch()
        {
            Assert.AreEqual(TokenKind.RegexMatch, Tokenizer.Tokenize("%=").First().Kind);
        }

        [Test]
        public void TildeTilde()
        {
            Assert.AreEqual(new[] { Token.Tilde(), Token.Tilde() }, Tokenizer.Tokenize("~~").Take(2).ToArray());
        }

        [Test]
        public void DashMatch()
        {
            Assert.AreEqual(TokenKind.DashMatch, Tokenizer.Tokenize("|=").First().Kind);
        }

        [Test]
        public void Pipe()
        {
            Assert.AreEqual(new[] { Token.Char('|'), Token.Char('|') }, Tokenizer.Tokenize("||").Take(2).ToArray());
        }

        [Test]
        public void StringSingleQuoted()
        {
            Assert.AreEqual(Token.String("foo bar"), Tokenizer.Tokenize("'foo bar'").First());
        }

        [Test]
        public void StringDoubleQuoted()
        {
            Assert.AreEqual(Token.String("foo bar"), Tokenizer.Tokenize("\"foo bar\"").First());
        }

        [Test]
        public void StringDoubleQuotedWithEscapedDoubleQuotes()
        {
            Assert.AreEqual(Token.String("foo \"bar\" baz"), Tokenizer.Tokenize("\"foo \\\"bar\\\" baz\"").First());
        }

        [Test]
        public void StringSingleQuotedWithEscapedSingleQuotes()
        {
            Assert.AreEqual(Token.String("foo 'bar' baz"), Tokenizer.Tokenize(@"'foo \'bar\' baz'").First());
        }

        [Test]
        public void StringDoubleQuotedWithEscapedBackslashes()
        {
            Assert.AreEqual(Token.String(@"foo \bar\ baz"), Tokenizer.Tokenize("\"foo \\\\bar\\\\ baz\"").First());
        }

        [Test]
        public void StringSingleQuotedWithEscapedBackslashes()
        {
            Assert.AreEqual(Token.String(@"foo \bar\ baz"), Tokenizer.Tokenize(@"'foo \\bar\\ baz'").First());
        }

        [Test]
        public void BracketedIdent()
        {
            var token = Tokenizer.Tokenize("[foo]").GetEnumerator();
            Assert.IsTrue(token.MoveNext()); Assert.AreEqual(Token.LeftBracket(), token.Current);
            Assert.IsTrue(token.MoveNext()); Assert.AreEqual(Token.Ident("foo"), token.Current);
            Assert.IsTrue(token.MoveNext()); Assert.AreEqual(Token.RightBracket(), token.Current);
            Assert.IsTrue(token.MoveNext()); Assert.AreEqual(Token.Eoi(), token.Current);
            Assert.IsFalse(token.MoveNext());
        }
        
        [Test]
        public void BadHash()
        {
            Assert.Throws<FormatException>(() => Tokenizer.Tokenize("#").ToArray());
        }

        [Test]
        public void HashDelimitedCorrectly()
        {
            var token = Tokenizer.Tokenize("#foo.").GetEnumerator();
            Assert.IsTrue(token.MoveNext()); Assert.AreEqual(Token.Hash("foo"), token.Current);
            Assert.IsTrue(token.MoveNext()); Assert.AreEqual(Token.Dot(), token.Current);
            Assert.IsTrue(token.MoveNext()); Assert.AreEqual(Token.Eoi(), token.Current);
            Assert.IsFalse(token.MoveNext());
        }

        [Test]
        public void Function()
        {
            Assert.AreEqual(Token.Function("funky"), Tokenizer.Tokenize("funky(").First());
        }

        [Test]
        public void FunctionWithEnclosedIdent()
        {
            var token = Tokenizer.Tokenize("foo(bar)").GetEnumerator();
            Assert.IsTrue(token.MoveNext()); Assert.AreEqual(Token.Function("foo"), token.Current);
            Assert.IsTrue(token.MoveNext()); Assert.AreEqual(Token.Ident("bar"), token.Current);
            Assert.IsTrue(token.MoveNext()); Assert.AreEqual(Token.RightParenthesis(), token.Current);
            Assert.IsTrue(token.MoveNext()); Assert.AreEqual(Token.Eoi(), token.Current);
            Assert.IsFalse(token.MoveNext());
        }

        [Test]
        public void Integer()
        {
            Assert.AreEqual(Token.Integer("42"), Tokenizer.Tokenize("42").First());
        }

        [Test]
        public void IntegerEnclosed()
        {
            var token = Tokenizer.Tokenize("[42]").GetEnumerator();
            Assert.IsTrue(token.MoveNext()); Assert.AreEqual(Token.LeftBracket(), token.Current);
            Assert.IsTrue(token.MoveNext()); Assert.AreEqual(Token.Integer("42"), token.Current);
            Assert.IsTrue(token.MoveNext()); Assert.AreEqual(Token.RightBracket(), token.Current);
            Assert.IsTrue(token.MoveNext()); Assert.AreEqual(Token.Eoi(), token.Current);
            Assert.IsFalse(token.MoveNext());
        }

        [Test]
        public void SubstringMatch()
        {
            Assert.AreEqual(TokenKind.SubstringMatch, Tokenizer.Tokenize("*=").First().Kind);
        }

        [Test]
        public void Star()
        {
            Assert.AreEqual(Token.Char('*'), Tokenizer.Tokenize("*").First());
        }

        [Test]
        public void StarStar()
        {
            Assert.AreEqual(new[] { Token.Char('*'), Token.Char('*') }, Tokenizer.Tokenize("**").Take(2).ToArray());
        }

		[Test]
		public void Tilde()
		{
			Assert.AreEqual(TokenKind.Tilde, Tokenizer.Tokenize("~").First().Kind);
		}

		[Test]
        public void TildeWhitespacePrepended()
		{
			Assert.AreEqual(TokenKind.Tilde, Tokenizer.Tokenize("  ~").First().Kind);
		}

        [Test]
        public void StringSingleQuoteUnterminated()
        {
            Assert.Throws<FormatException>(() => Tokenizer.Tokenize("'foo").ToArray());
        }

        [Test]
        public void StringDoubleQuoteUnterminated()
        {
            Assert.Throws<FormatException>(() => Tokenizer.Tokenize("\"foo").ToArray());
        }

        [Test]
        public void StringInvalidEscaping()
        {
            Assert.Throws<FormatException>(() => Tokenizer.Tokenize(@"'f\oo").ToArray());
        }

        [Test]
        public void NullReader()
        {
            Assert.Throws<ArgumentNullException>(() => Tokenizer.Tokenize((TextReader) null));
        }

        [Test]
        public void StringReader()
        {
            Assert.AreEqual(new[] { Token.Integer("123"), Token.Comma(), Token.Char('*'), Token.Eoi() }, 
                Tokenizer.Tokenize(new StringReader("123,*")).ToArray());
        }

        [Test]
        public void InvalidChar()
        {
            Assert.Throws<FormatException>(() => Tokenizer.Tokenize("what?").ToArray());
        }
    }
}