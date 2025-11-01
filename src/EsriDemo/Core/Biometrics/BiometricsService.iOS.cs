using System.Diagnostics.CodeAnalysis;
using Plugin.Maui.Biometric;

namespace EsriDemo.Core.Biometrics;

[ExcludeFromCodeCoverage]
public class BiometricsService : IBiometricsService
{
    public async Task<BiometricHwStatus> GetAuthenticationStatusAsync(AuthenticatorStrength authStrength = AuthenticatorStrength.Strong)
    {
        var result = await BiometricAuthenticationService.Default.GetAuthenticationStatusAsync(authStrength.ToAuthenticatorStrength());
        return result.ToBiometricHwStatus();
    }

    public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request, CancellationToken token)
    {
        var result = await BiometricAuthenticationService.Default.AuthenticateAsync(request.ToAuthenticationRequest(), token);
        return result.ToAuthenticationResponse();
    }

    public async Task<BiometricType[]> GetSupportedBiometricTypesAsync()
    {
        var result = await BiometricAuthenticationService.Default.GetEnrolledBiometricTypesAsync();
        return result.ToBiometricTypeArray();
    }

    public bool IsPlatformSupported => BiometricAuthenticationService.Default.IsPlatformSupported;
}