namespace EsriDemo.Core.Biometrics;

public interface IBiometricsService
{
    Task<BiometricHwStatus> GetAuthenticationStatusAsync(AuthenticatorStrength authStrength = AuthenticatorStrength.Strong);

    Task<AuthenticationResponse> AuthenticateAsync(
        AuthenticationRequest request,
        CancellationToken token);

    Task<BiometricType[]> GetSupportedBiometricTypesAsync();

    bool IsPlatformSupported { get; }
}