# ASP.NET Core Template Pack

[![Build status](https://ci.appveyor.com/api/projects/status/2txy5hi1ac7jima6?svg=true)](https://ci.appveyor.com/project/madskristensen/aspnetcoretemplatepack)

Contains the following project templates for ASP.NET Core development:

- Static website template
- MVC Basic template
- MVC Fast template
- Blog application
- Photo Gallery application

## Install

### Command line
In the console, execute the following command:

> dotnet new -i "MadsKristensen.AspNetCore.Web.Templates::\*"

### Visual Studio 2017.3
The extension adds the project template to the ASP.NET New Project dialog:

![New Project Dialog](art/new-project-dialog.png)

## Templates

### Static Website
Uses the ASP.NET Core project system to provide the latest features in Visual Studio web tooling to create a completely static website. No C# in the project at all - just plain old CSS, JavaScript and HTML.

![Static Web Screenshot](art/static-web-screenshot.png)

## Custom MimeType mappings
See the [Static Site Helper repo](https://github.com/madskristensen/AspNetCore.StaticSiteHelper) for how to serve custom file extensions at development time through ASP.NET Core.

#### Use

> dotnet new staticweb -n myapp

### MVC Basic
This template makes it super easy to get started with building an ASP.NET Core MVC application. It doesn't have any dependencies on Bower, npm, BundlerMinifier, Bootstrap, jQuery or anything else. It's the perfect starting point for developers that know their ASP.NET Core.

![Mvc Basic Screenshot](art/mvc-basic-screenshot.png)

#### Use

> dotnet new mvcbasic -n myapp

### MVC Fast
This template is a variation of the *MVC Basic* template, but with added features for creating high performance web applications.

**Features**:

- Using Gulp to bundle and minify CSS and JS files
- Minifies the HTML
- Uses response caching on both client and server
- Inline CSS for *above the fold* content
- Cache busting of CSS and JS references

![Mvc Fast Screenshot](art/mvc-fast-screenshot.png)

The template points starts you out with the best score on [webpagetest.org](http://webpagetest.org).

![Speed test](art/mvc-basic-speedtest.png)

As well as 100/100 points on [PageSpeed Insights](https://developers.google.com/speed/pagespeed/insights/)

![PageSpeed Insights](art/mvc-basic-pagespeed.png)

#### Use

> dotnet new mvcfast -n myapp && npm install

### Blog application
This is the [Miniblog.Core](https://github.com/madskristensen/Miniblog.Core) application - a high performant and full featured blogging app.

### Photo Gallery application
This is the [Photo Gallery](https://github.com/madskristensen/PhotoGallery) application - A photo gallery site implemented in ASP.NET Core 2.0 Razor Pages.