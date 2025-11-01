using System.Diagnostics.CodeAnalysis;

namespace EsriDemo.Core.Biometrics;

[ExcludeFromCodeCoverage]
public class BiometricsService : IBiometricsService
{
    public Task<BiometricHwStatus> GetAuthenticationStatusAsync(AuthenticatorStrength authStrength = AuthenticatorStrength.Strong)
    {
        throw new NotImplementedException();
    }

    public Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<BiometricType[]> GetSupportedBiometricTypesAsync()
    {
        throw new NotImplementedException();
    }

    public bool IsPlatformSupported => throw new NotImplementedException();
}