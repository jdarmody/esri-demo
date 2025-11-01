namespace EsriDemo.Core.Biometrics;

public enum BiometricHwStatus
{
    NoHardware,
    Unavailable,
    Unsupported,
    NotEnrolled,
    LockedOut,
    Success,
    Failure,
    PresentButNotEnrolled,
}