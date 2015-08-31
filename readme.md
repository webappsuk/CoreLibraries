Web Applications UK Ltd Utilities
=================================

This library contains a treasure trove of utilities built up over a decade by [Web Applications UK] (http://www.webappuk.com) for simplifying the building and deployment of enterprise scale .NET solutions.  The code has been successfully used, in various forms, for a number of years in production environments driving millions of transactions.

As part of our ethos to have a positive impact on our wider community we are open sourcing them for everyone's benefit.  We've also included the last three years of development history, showing how the utilities have evolved.  Please let us know if you find the code useful in your own projects!

We have carefully sanitised the code to protect our commercial customers interests, but if you do manage to spot any sensitive data please notify us directly at support@webppauk.com.

Using the libraries
-------------------

All the code present [here] (https://github.com/webappsuk/CoreLibraries) is also available on [nuget.org] (https://www.nuget.org/packages?q=WebApplicationsUK), as a collection of 10 inter-connected NuGets and can be installed into your .NET projects using NuGet.

The official NuGets are strongly named and digitally signed by [Web Applications UK] (http://www.webappuk.com), using a separate, secure process, that gaurantees the NuGets authenticity.  Although you can build your own NuGets from the source included [here] (https://github.com/webappsuk/CoreLibraries), the ability to upload them to [nuget.org] (https://www.nuget.org/packages?q=WebApplicationsUK) is not provided to protect casual users.  If you wish to use a customised version of a library, we recommend building and including the dll directly (but remember to include the [License](license.md)!).

We will accept relevant pull requests, and issues, so please consider [contributing](CONTRIBUTING.md).

Building the libraries
----------------------

The libraries are built using [Visual Studio 2015] (https://www.visualstudio.com/en-us/downloads/download-visual-studio-vs.aspx).  They can also be built using msbuild, and for that reason build dependencies are included as project references in projects (see ["Incorrect solution build ordering when using msbuild.exe"] (http://blogs.msdn.com/b/visualstudio/archive/2010/12/21/incorrect-solution-build-ordering-when-using-msbuild-exe.aspx)).

We are continually migrating the code to the latest versions of .NET and C#, but this is a rolling procees.  If there's some code that you particularly would like to see working in a previous version of .NET then you can try looking at the history of the files to see how they've changed through each version.

Documentation
-------------

The libraries are self-documenting and provide useful intellisense, this should also be accessible using [nudoq.org] (http://www.nudoq.org/#!/Projects/WebApplications).  There are some useful blogs on [thargy.com] (http://thargy.com/category/dev/csharp/), and we hope to provide more tutorials and examples if the NuGets prove popular.

License
-------

See [License](license.md).

Contributing
------------

See [Contributing](CONTRIBUTING.md).
