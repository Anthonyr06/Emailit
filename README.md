<div id="top"></div>

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]



<br />
<div align="center">
  <a href="https://github.com/Anthonyr06/Emailit">
    <img src="images/Logo.png" alt="Logo" width="80" height="80">
  </a>

<h3 align="center">Emailit</h3>

  <p align="center">
    Emailit is a software system made to communicate between employees of a company.
    <br />
    <a href="https://github.com/Anthonyr06/Emailit"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/Anthonyr06/Emailit">View Demo</a>
    ·
    <a href="https://github.com/Anthonyr06/Emailit/issues">Report Bug</a>
    ·
    <a href="https://github.com/Anthonyr06/Emailit/issues">Request Feature</a>
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>




&nbsp;
## About The Project
&nbsp;
&nbsp;
![Product Name Screen Shot][product-screenshot]

Emailit is a software system made to communicate between employees of a company. You can send and received messages from users registered in the system. The system has the same functions as a common email system with some special features.

The objective of this project is to provide an email like system to companies where the privacy is the first.

Although the first principle of Emailit is to send and received messages, you can perform more process as well, as managing users permissions, audit user's sessions and modifications, manage departments and branch offices and so on.

<p align="right">(<a href="#top">back to top</a>)</p>



### Built With

* [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps)
* [HtmlAgilityPack](https://html-agility-pack.net/)
* [MailKit](http://www.mimekit.net/)
* [Bootstrap](https://getbootstrap.com)
* [JQuery](https://jquery.com)
* [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
* [SignalR](https://dotnet.microsoft.com/en-us/apps/aspnet/signalr)
* [Animate.css](https://animate.style/)
* [HtmlSanitizer](https://github.com/mganss/HtmlSanitizer)



<p align="right">(<a href="#top">back to top</a>)</p>




## Getting Started
&nbsp;

### Prerequisites

* .Net core 3.1
* SQL Server


### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/Anthonyr06/Emailit.git
   ```

2. Enter your SMTP data in `appsettings.json`
   ```json 
   "EmailConfiguration": {
      "From": "example@outlook.com",
      "SmtpServer": "smtp.office365.com",
      "Port": 587,
      "Username": "example@outlook.com",
      "Password": "P@SSW0rd"
    },

   ```

3. Enter a private key for JWT tokens in `appsettings.json` (This key is used to sign the forgot password tokens)
   ```json 
   "TokenProvider": {
      "Key": "MlJQpiwhyNtwsV5Ke4LM59xs2TpPOnwYj",
      ```
4. Run the project! 
 
    As soon as you do it, the migrations inside `/Data/Migrations` will apply then a new Database is going to be created, and some data is going to be seeded.




<p align="right">(<a href="#top">back to top</a>)</p>



## Usage
&nbsp;


Following the firts run, you're going to be able to login using the same email you used in `EmailConfiguration` from `appsettings.json`. The password is going to be `00000000000` (11 digits).

![first login][first-login-gif]

Every time a new user is created, the user must change the password after first login. 

&nbsp;
### Sending and receiving messages


If you want to send a new message, just click the top left corner button, and fill the form inside the modal. Write down as desired and attach some files if you want :)

![sending a message][send-message-gif]

As shown in the GIF, after sending the message, the recipients received a notification. (in this case, the user sent the message to himself and another user).

&nbsp;
### Recovering forgotten password
There are two options if the user forgot the password:
1. An admin activate the Must Change Password flag, then the user would be able to change the password after next login
2. The user recovers the password by itself using its private email. (When registering a user it's recommended to use a valid email address thus if a user forget its password, can recover it by itself without admin help).

![Recovering password][recover-pwd-gif]



<p align="right">(<a href="#top">back to top</a>)</p>



## Roadmap

- [x] Transcribe project from Spanish to English
- [ ] Rework front end with VueJS
- [ ] Make app multi-language with .Net Core Localization


See the [open issues](https://github.com/Anthonyr06/Emailit/issues) for a full list of proposed features (and known issues).

<p align="right">(<a href="#top">back to top</a>)</p>



## License

Distributed under the GNU GPLv3 License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#top">back to top</a>)</p>



## Contact

Anthony - https://linkedin.com/in/arg06

Project Link: [https://github.com/Anthonyr06/Emailit](https://github.com/Anthonyr06/Emailit)

<p align="right">(<a href="#top">back to top</a>)</p>


## Acknowledgments

* [Build Bundler Minifier](https://github.com/madskristensen/BundlerMinifier/)
* [Inputmask](https://github.com/RobinHerbots/Inputmask)
* [JQuery Validation](https://jqueryvalidation.org/)
* [MagicSuggest](http://nicolasbize.com/magicsuggest/)
* [Summernote](https://summernote.org/)

<p align="right">(<a href="#top">back to top</a>)</p>


[contributors-shield]: https://img.shields.io/github/contributors/Anthonyr06/Emailit?style=for-the-badge
[contributors-url]: https://github.com/Anthonyr06/Emailit/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/Anthonyr06/Emailit?style=for-the-badge
[forks-url]: https://github.com/Anthonyr06/Emailit/network/members
[stars-shield]: https://img.shields.io/github/stars/Anthonyr06/Emailit?style=for-the-badge
[stars-url]: https://github.com/Anthonyr06/Emailit/stargazers
[issues-shield]: https://img.shields.io/github/issues/Anthonyr06/Emailit?style=for-the-badge
[issues-url]: https://github.com/Anthonyr06/Emailit/issues
[license-shield]: https://img.shields.io/github/license/Anthonyr06/Emailit?style=for-the-badge
[license-url]: https://github.com/Anthonyr06/Emailit/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/arg06
[product-screenshot]: images/overview.gif
[first-login-gif]:images/firstLogin.gif
[send-message-gif]:images/sendMessage.gif
[recover-pwd-gif]:images/recoverPassword.gif
