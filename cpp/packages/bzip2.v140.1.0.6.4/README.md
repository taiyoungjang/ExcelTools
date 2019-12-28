# BZIP2.V140

This package contains the bzip2 debug and release DLLs, the corresponding import libraries and the bzip2 header file. It was built with Visual Studio 2015 (V140).

## Source Code

The source code used to build this package is available at http://www.bzip.org/1.0.6/bzip2-1.0.6.tar.gz.

## Build Instructions

Follow these instructions to rebuild this package from sources:

### Download

Download the bzip2 source archive, then unpack this archive.

### Patch

Download the following patch: https://zeroc.com/download/bzip2/patch.bzip2-1.0.6

This patch adds support for building bzip2 as a DLL on Windows.

Apply this patch from the main directory of the bzip2 source distribution:

``patch -p0 < C:\users\%username%\Downloads\patch.bzip2-1.0.6``

You can download a patch utility for Windows from http://gnuwin32.sourceforge.net/packages/patch.htm.
On Windows 7 or later, UAC can make it difficult to use the patch utility unless you take extra steps.

One solution is to run ``patch.exe`` in a command window that you started with Administrator privileges
(right-click on Command Prompt in the Start menu and choose ``Run as administrator``). 

If running as administrator is not an option, do the following:

1. Do not install patch.exe in a system-protected directory such as ``C:\Program Files``.
2. Update the manifest in ``patch.exe`` as described at http://math.nist.gov/oommf/software-patchsets/patch_on_Windows7.html.

### Build

- Open a Visual Studio 2012 command prompt.

- Change to the main directory of the bzip2 source distribution and type:
	
	``nmake /f Makefile.mak``
