### Dotnet SDK, workload, and Maui versions
To ensure the development team and the GitHub actions are all using the same dotnet sdk, maui workload version, and maui version we have the following:

- Maui version: Maintain `<MauiVersion>` in the .csproj.

- Global.json: Maintain the dotnet sdk and workload version in [global.json](src/global.json).

- GitHub Actions Repository Variables: Set the variables `DOTNET_VERSION` and `MAUI_WORKLOAD_VERSION` to match the values in global.json to have deterministic builds.

## Github Actions
### Git branching strategy
This repository relies on 3 branches:
- develop
- test
- main

`develop` is where all the latest development is merged into and Dev releases are created when pull requests are merged into it as part of following the practice of CI/CD.

`test` is used for creating Test (UAT) releases and `main` is used for creating production releases.

### Environments
There are 3 environments setup in GitHub Actions:
- development
- test
- production

This helps in setting up environment specific secrets and variables for the app releases. This plays into utilising GitHub's `Deployments` feature, which tracks the releases created per environment.

The purpose of development releases is to allow for developers to test the app anytime (but could be shared with any tester), and test releases are official releases that the team wish to distribute for UAT.

The main differences between the 2 are:
- Application ID
- App title
- Build environment constant (see variable DEFINE_CONSTANTS)
- Firebase app ID
- Dev release will include Debug level logs

Both test and development releases log to the console, which can be tracked by connecting the device to a computer. For example, to a Mac and use the Mac Console app to see iOS logs or Android Studio to view the logcat logs from the app.
Production apps do not log to the console.

### Secrets
As documented by GitHub, secret files (e.g. certificates or json) must be a Base64 format, which can be achieved with this script: `base64 -i SECRET_FILE_NAME | pbcopy`

#### Repository Secrets
There are several repository secrets for GitHub Actions required for both workflows.

- ANDROID_KEYSTORE_* secrets: the one Android keystore is used for all builds, hence, the ANDROID_KEYSTORE_* secrets are used for all workflows and environments.
- FIREBASE_CREDENTIAL_FILE_CONTENT: provides credentials for secure access to distribute to Firebase App Distribution.
- ANDROID_GOOGLE_SERVICES_JSON_BASE64: the single json config file needed for all environments. This can be downloaded from Firebase app settings.
- IOS_GOOGLE_SERVICE_INFO_PLIST_BASE64: this is the same as the `development` environment secret, but is used for the pull request checks workflow.
- IOS_P12_CERT, IOS_P12_PASSWORD, IOS_PROVISION_PROFILE_BASE64: are again the same as the `development` environment secrets, but is used for the pull request checks workflow.

#### Environment Secrets
- ANDROID_PLAYSTORE_SERVICE_ACCOUNT: Production ONLY for uploading to Google Play Store.
- IOS_APPSTORE_ISSUER_PRIVATE_KEY: Production ONLY for uploading to TestFlight.
- IOS_GOOGLE_SERVICE_INFO_PLIST_BASE64: Firebase config file.
- IOS_P12_CERT: Base64 format of the p12 certificate to sign the iOS app.
- IOS_P12_PASSWORD: Password for the p12 certificate.
- IOS_PROVISION_PROFILE_BASE64: Base64 format of the Provisioning profile to sign the iOS app.

### Variables
#### Repository variables
These should all be self-explanatory when you see the variable names and values.

Below are the current variables used by both workflows:

- APP_PROJECT_PATH: Path to the .csproj file of the app.
- BUILD_CONFIGURATION: Always build in Release.
- DOTNET_VERSION: version used to install the dotnet SDK. See more details [here](https://github.com/actions/setup-dotnet/tree/v4/).
- DOTNET_VERSION_TARGET: used in the dotnet publish step e.g. `net9.0` will be used as `net9.0-ios` and `net9.0-android` in the dotnet publish commands.
- GOOGLE_SERVICES_JSON_PATH: the path where the google firebase config secret file for Android would be copied to.
- GOOGLE_SERVICE_INFO_PLIST_PATH: the path where the google firebase config secret file for iOS would be copied to.
- MAUI_WORKLOAD_VERSION: set the version for the maui-mobile workload. Setting the `DOTNET_VERSION` and `MAUI_WORKLOAD_VERSION` helps to have deterministic builds. In addition to this, we have the [global.json](src/global.json) as well, which will have the same dotnet sdk and dotnet workload versions.
- MAC_OS_AGENT_VERSION: the MacOS version to use for the GitHub workflows.
- TEST_PROJECT_PATH: the path to .csproj file of the test project.
- XCODE_VERSION: the Xcode version required to build the iOS app.

#### Environment variables
These should all be self-explanatory when you see the variable names and values.

Below are the current environment variables:

- ANDROID_APPLICATION_ID: the android package/application ID.
- ANDROID_FIREBASE_APP_ID: the android app ID in Firebase settings.
- ANDROID_PACKAGE_FORMATS: package format(s) to use when building the android app.
- APPLICATION_TITLE: the app title.
- DEFINE_CONSTANTS: environment constant used in the dotnet publish CLI command i.e. -p:DefineConstants
- IOS_APPLICATION_ID: the iOS bundle/application ID.
- IOS_APPSTORE_ISSUER: the issuer for the App Store service connection.
- IOS_APPSTORE_ISSUER_KEYID: the key id for the App Store service connection.
- IOS_CODESIGN_KEY: the Apple Distribution certificate name. This helps the dotnet publish locate the p12 certificate installed to sign the app.
- IOS_FIREBASE_APP_ID: the iOS app ID in Firebase settings.

### Pull Request workflow
See [Pull Request Checks workflow](.github/workflows/pull_request_checks.yml) yaml.

For every pull request, this workflow will be triggered. It's purpose is to ensure all tests pass and the app continues to build successfully.

### Build and Deploy workflow
See [Build and Deploy workflow](.github/workflows/build_deploy.yml) yaml for a full understanding of how the workflow operates.

This will be triggered by merges to develop, test, or main git branches OR a manual run from GitHub Actions on any branch.

This workflow will run the tests and then build both the iOS and Android releases for the appropriate environment based on the git branch it is triggered on. It will default to the development environment.

After a successful build it will deploy to Firebase for development and test releases or the stores testing tracks for production.

#### GitHub Releases and Semantic versioning based on Git history
The workflow will determine the display version of the release based on git tags and git commits. The tags are created via creating a GitHub Release in the workflow after a successful builds of iOS and Android.

For development releases, the workflow uses the GitHub Action [semantic-version](https://github.com/paulhatch/semantic-version/) to determine the application display version. This is a simplified way compared to other options where the major and minor incrementing can be controlled via a commit message using the configured pattern specified in the workflow step. Currently the patterns expected are:
- Major increment pattern: `(MAJOR)`
- Minor increment pattern: `(MINOR)`

The patch number is then determined by the number of commits since the last tag on the branch. Normally, this will just be 1 commit since the last git tag (the commit from the merge that triggered the build).

The development and test release display versions will include a suffix of -dev and -test respectively.

For test and production builds, the semantic version is simply determined by the last version tag from the source branch. For example, if merging develop to test, the develop branch last tag being v1.2.3-dev, the test version will be 1.2.3-test. Then when merging test to main, the production release version would be 1.2.3.

The application version code is determined by the timestamp of the workflow run and  in the format of 'YYYYMMDD[workflow run count]' e.g. 2025081502 = the 2nd workflow run on 15th Aug 2025.

The GitHub releases help with tracking releases, generating release notes, and can also download the release artifacts if required.

For test and production releases, the workflow will check the description in the pull request used to create the release. The pull request description can be used to include release summary notes. These will be included in the GitHub Release notes, Firebase App Distribution release notes, and Google Play production "what's new" notes.

## Release process
Follow the steps below for building releases per environment.

Manual GitHub Action runs can be triggered on any branch if required. Will default to development environment release if not the main or test branch.

### Development
Raise a pull request targeting the `develop` branch. The merge will automatically trigger a dev build. Notice the version patch number automatically increments during the GitHub workflow run unless a major or minor change is determined. Ensure to include a (MAJOR) or (MINOR) in any of the commit messages (or the final merge commit) if the major or minor numbers need to be incremented.

A firebase release will be distributed to the 'dev' tester group.


### Test (UAT)
Merge `develop` to `test` via a pull request. Optionally include a release notes summary using the pull request description. A firebase release will be distributed to the 'dev' tester group. Manually distribute in Firebase App Distribution to the 'uat' tester group or any tester when required.

### Production
Merge `test` to `main` via a pull request. Optionally include a release notes summary using the pull request description.

The production builds will deploy to Apple TestFlight and Google Play Internal Testing.
