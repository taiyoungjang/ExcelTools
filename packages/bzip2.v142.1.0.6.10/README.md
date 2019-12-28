# BZIP2.V142

This package contains the bzip2 debug and release DLLs, the corresponding import libraries and the bzip2 header file. It was built with Visual Studio 2019 (V142).

## Source Code

The source code used to build this package is available at https://github.com/zeroc-ice/bzip2.

## Build Instructions
```
git clone git@github.com:zeroc-ice/bzip2.git -b msvc
cd bzip2
MSBuild msbuild\bzip2.proj /t:NugetPack
```
