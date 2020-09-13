using System;
using Xunit;

namespace Client.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var c = new Class3();

            var result = c.Plus(1, 2);

            Assert.Equal(3, result);
        }
    }
}