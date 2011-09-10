# Overview
StructureAssertions is a library that can be used to enforce architectural structure of your application in unit tests.

# Installing
The simplest way to use this library is to install NuGet package [StructureAssertions](http://nuget.org/List/Packages/StructureAssertions). Simply execute this command from Visual Studio Package Manager Console:

    Install-Package StructureAssertions

# Usage
You can use this library to check dependencies of a group of classes. For example, in ASP.NET MVC application, you want to enforce that none of your Model classes reference any of your Controller classes. You can enforce this architectural rule by unit test like this:

    [Test]
    public void ModelsDoNotReferenceControllers2()
    {
        AssertStructure
            .InAssemblyContaining<UserController>()
            .Types(type => type.Namespace.EndsWith(".Models"))
            .MustNotReference(reference => reference.Namespace.EndsWith(".Controllers"));
    }

It checks that all Model classes from assembly containing class UserController must not reference any Controller class; otherwise, an exception is thrown. Selecting assembly by contained type (UserController) is just convenient method how to find an assembly, you can pass Assembly object as well.

# How it works
Specified assembly is loaded and analyzed using library [Mono.Cecil](http://www.mono-project.com/Cecil). Classes that conform to expression supplied in Types() method are analyzed for all possible dependencies. Dependency is any usage of other class, wherever in the code of the class. Found dependencies are checked with expression supplied to MustNotReference() method, and if any forbidden dependency is found, a ForbiddenDependencyException is thrown.