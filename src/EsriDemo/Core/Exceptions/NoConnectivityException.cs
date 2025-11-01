using System.Diagnostics.CodeAnalysis;

namespace EsriDemo.Core.Exceptions;

[ExcludeFromCodeCoverage]
public class NoConnectivityException() : Exception("Strings.ConnectionErrorMessage"); //TODO