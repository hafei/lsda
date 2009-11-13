// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestInterceptor.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The test interceptor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel.Interceptors
{
    using Extended.Interceptors;

    /// <summary>
    /// The test interceptor.
    /// </summary>
    public class TestInterceptor : QueryInterceptor<ITestScope>
    {
    }
}