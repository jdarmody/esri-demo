using Moq.AutoMock;
using System.Globalization;

namespace EsriDemo.UnitTests;

public abstract class UnitTestBase<TSut> : UnitTestBase where TSut : class
{
    protected UnitTestBase()
    {
        Sut = Mocker.CreateInstance<TSut>();
    }

    protected TSut Sut { get; }
}

public abstract class UnitTestBase
{
    protected UnitTestBase()
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-AU");
        Mocker = new AutoMocker();
    }

    protected AutoMocker Mocker { get; }
}