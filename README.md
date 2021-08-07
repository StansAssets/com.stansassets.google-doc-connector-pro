# Google Doc Connector Pro
The plugin allows you to use Google Sheets as a configuration repository for your game. And also use Google Sheets as a repository for localization.
The plugin saves data from spreadsheets in json and uses data from them when building a project. In the Editor mode, you can synchronize and save all the necessary spreadsheets.

[![NPM Package](https://img.shields.io/npm/v/com.stansassets.google-doc-connector-pro)](https://www.npmjs.com/package/com.stansassets.google-doc-connector-pro)
[![openupm](https://img.shields.io/npm/v/com.stansassets.google-doc-connector-pro?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.stansassets.google-doc-connector-pro/)
[![Licence](https://img.shields.io/npm/l/com.stansassets.google-doc-connector-pro)](https://github.com/StansAssets/com.stansassets.google-doc-connector-pro/blob/master/LICENSE)
[![Issues](https://img.shields.io/github/issues/StansAssets/com.stansassets.google-doc-connector-pro)](https://github.com/StansAssets/com.stansassets.google-doc-connector-pro/issues)


[API Reference](https://api.stansassets.com/foundation/StansAssets.Foundation.html)

### Install from NPM
* Navigate to the `Packages` directory of your project.
* Adjust the [project manifest file](https://docs.unity3d.com/Manual/upm-manifestPrj.html) `manifest.json` in a text editor.
* Ensure `https://registry.npmjs.org/` is part of `scopedRegistries`.
  * Ensure `com.stansassets` is part of `scopes`.
  * Add `com.stansassets.google-doc-connector-pro` to the `dependencies`, stating the latest version.

A minimal example ends up looking like this. Please note that the version `X.Y.Z` stated here is to be replaced with [the latest released version](https://www.npmjs.com/package/com.stansassets.google-doc-connector-pro) which is currently [![NPM Package](https://img.shields.io/npm/v/com.stansassets.google-doc-connector-pro)](https://www.npmjs.com/package/com.stansassets.google-doc-connector-pro).
  ```json
  {
    "scopedRegistries": [
      {
        "name": "npmjs",
        "url": "https://registry.npmjs.org/",
        "scopes": [
          "com.stansassets"
        ]
      }
    ],
    "dependencies": {
      "com.stansassets.google-doc-connector-pro": "X.Y.Z",
      ...
    }
  }
  ```
* Switch back to the Unity software and wait for it to finish importing the added package.

### Install from OpenUPM
* Install openupm-cli `npm install -g openupm-cli` or `yarn global add openupm-cli`
* Enter your unity project folder `cd <YOUR_UNITY_PROJECT_FOLDER>`
* Install package `openupm add com.stansassets.google-doc-connector-pro`

### Install from a Git URL
Yoy can also install this package via Git URL. To load a package from a Git URL:

* Open [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui.html) window.
* Click the add **+** button in the status bar.
* The options for adding packages appear.
* Select Add package from git URL from the add menu. A text box and an Add button appear.
* Enter the `https://github.com/StansAssets/com.stansassets.google-doc-connector-pro.git` Git URL in the text box and click Add.
* You may also install a specific package version by using the URL with the specified version.
  * `https://github.com/StansAssets/com.stansassets.google-doc-connector-pro.git#X.Y.X`
  * Please note that the version `X.Y.Z` stated here is to be replaced with the version you would like to get.
  * You can find all the available releases [here](https://github.com/StansAssets/com.stansassets.google-doc-connector-pro/releases).
  * The latest available release version is [![Last Release](https://img.shields.io/github/v/release/stansassets/com.stansassets.google-doc-connector-pro)](https://github.com/StansAssets/com.stansassets.google-doc-connector-pro/releases/latest)

### Referencing packages from private repositories by SSH
Unity Package Manager supports referencing packages from private repositories by SSH, see an example:
`"com.company.app": "ssh://git@github.com/Company/app.git#X.Y.Z"`.
But easy to stumble if doing it the first time because Unity Package Manager requires the only SSH key without a passphrase. A quick guide to starting with:
* Start Git Bash and generate SSH key without a passphrase if you don't have one already. ([Git SSH Guide](https://help.github.com/en/github/authenticating-to-github/generating-a-new-ssh-key-and-adding-it-to-the-ssh-agent))
* Add SSH key to an ssh-agent. Reboot PC.
* Create some temporary directory on a PC and go there by executing command `cd C:\Repositories\SSHTestRepository`. Clone repository that going to be referenced as a package with `git clone git@github.com/Company/app.git`, agree with adding to a hosts list.
* Done! Feel free to delete previously cloned repository.

For more information about what protocols Unity supports, see [Git URLs](https://docs.unity3d.com/Manual/upm-git.html).

## About Us
We are committed to developing high quality and engaging entertainment software. Our mission has been to bring a reliable and high-quality Unity Development service to companies and individuals around the globe. 
At Stan's Assets, we make Plugins, SDKs, Games, VR & AR Applications. Do not hesitate do get in touch, whether you have a question, want to build something, or just to say hi :) [Let's Talk!](mailto:stan@stansassets.com)

[Website](https://stansassets.com/) | [LinkedIn](https://www.linkedin.com/in/lacost/) | [Youtube](https://www.youtube.com/user/stansassets/videos) | [Github](https://github.com/StansAssets) | [AssetStore](https://assetstore.unity.com/publishers/2256)
