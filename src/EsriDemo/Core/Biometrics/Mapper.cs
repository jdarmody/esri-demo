#if ANDROID || IOS
using System.Diagnostics.CodeAnalysis;

namespace EsriDemo.Core.Biometrics;

[ExcludeFromCodeCoverage]
public static class Mapper
{
    public static Plugin.Maui.Biometric.AuthenticatorStrength ToAuthenticatorStrength(
        this AuthenticatorStrength authStrength)
    {
        return authStrength switch
        {
            AuthenticatorStrength.Strong => Plugin.Maui.Biometric.AuthenticatorStrength.Strong,
            AuthenticatorStrength.Weak => Plugin.Maui.Biometric.AuthenticatorStrength.Weak,
            _ => throw new ArgumentOutOfRangeException(nameof(authStrength), authStrength, "Unsupported authenticator strength")
        };
    }
    
    public static BiometricHwStatus ToBiometricHwStatus(this Plugin.Maui.Biometric.BiometricHwStatus status)
    {
        return status switch
        {
            Plugin.Maui.Biometric.BiometricHwStatus.NoHardware => BiometricHwStatus.NoHardware,
            Plugin.Maui.Biometric.BiometricHwStatus.Unavailable => BiometricHwStatus.Unavailable,
            Plugin.Maui.Biometric.BiometricHwStatus.Unsupported => BiometricHwStatus.Unsupported,
            Plugin.Maui.Biometric.BiometricHwStatus.NotEnrolled => BiometricHwStatus.NotEnrolled,
            Plugin.Maui.Biometric.BiometricHwStatus.LockedOut => BiometricHwStatus.LockedOut,
            Plugin.Maui.Biometric.BiometricHwStatus.Success => BiometricHwStatus.Success,
            Plugin.Maui.Biometric.BiometricHwStatus.Failure => BiometricHwStatus.Failure,
            Plugin.Maui.Biometric.BiometricHwStatus.PresentButNotEnrolled => BiometricHwStatus.PresentButNotEnrolled,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unsupported biometric hardware status")
        };
    }
    
    
    public static Plugin.Maui.Biometric.AuthenticationRequest ToAuthenticationRequest(
        this AuthenticationRequest request)
    {
        return new Plugin.Maui.Biometric.AuthenticationRequest
        {
            Title = request.Title,
            Description = request.Description,
            NegativeText = request.NegativeText,
            AllowPasswordAuth = request.AllowPasswordAuth,
            AuthStrength = request.AuthStrength.ToAuthenticatorStrength(),
            Subtitle = request.Subtitle
        };
    }

    public static AuthenticationResponse ToAuthenticationResponse(
        this Plugin.Maui.Biometric.AuthenticationResponse response)
    {
        return new AuthenticationResponse
        {
            AuthenticationType = response.AuthenticationType.ToAuthenticationType(),
            ErrorMsg = response.ErrorMsg,
            Status = response.Status.ToBiometricResponseStatus()
        };
    }

    public static AuthenticationType ToAuthenticationType(
        this Plugin.Maui.Biometric.AuthenticationType authenticationType)
    {
        return authenticationType switch
        {
            Plugin.Maui.Biometric.AuthenticationType.Biometric => AuthenticationType.Biometric,
            Plugin.Maui.Biometric.AuthenticationType.DeviceCreds => AuthenticationType.DeviceCreds,
            Plugin.Maui.Biometric.AuthenticationType.Unknown => AuthenticationType.Unknown,
            Plugin.Maui.Biometric.AuthenticationType.WindowsHello => AuthenticationType.WindowsHello,
            _ => throw new ArgumentOutOfRangeException(nameof(authenticationType), authenticationType, "Unsupported biometric authentication type")
        };
    }

    public static BiometricResponseStatus ToBiometricResponseStatus(
        this Plugin.Maui.Biometric.BiometricResponseStatus biometricResponseStatus)
    {
        return biometricResponseStatus switch
        {
            Plugin.Maui.Biometric.BiometricResponseStatus.Failure => BiometricResponseStatus.Failure,
            Plugin.Maui.Biometric.BiometricResponseStatus.Success => BiometricResponseStatus.Success,
            _ => throw new ArgumentOutOfRangeException(nameof(biometricResponseStatus), biometricResponseStatus, "Unsupported biometric response status")
        };
    }

    public static BiometricType[] ToBiometricTypeArray(this Plugin.Maui.Biometric.BiometricType[] biometricTypes)
    {
        return biometricTypes.Select(ToBiometricType).ToArray();
    }

    public static BiometricType ToBiometricType(this Plugin.Maui.Biometric.BiometricType biometricTypes)
    {
        return biometricTypes switch
        {
            Plugin.Maui.Biometric.BiometricType.None => BiometricType.None,
            Plugin.Maui.Biometric.BiometricType.Fingerprint => BiometricType.Fingerprint,
            Plugin.Maui.Biometric.BiometricType.Face => BiometricType.Face,
            Plugin.Maui.Biometric.BiometricType.WindowsHello => BiometricType.WindowsHello,
            _ => throw new ArgumentOutOfRangeException(nameof(biometricTypes), biometricTypes, "Unsupported biometric type")
        };
    }
}
#endif