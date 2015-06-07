This is a fork of the MonoDevelop C/C++ binding, originally created
by using git subtree and git-mv-with-history to extract it from the
monodevelop repository.

You need libclang.so and CLangSharp.dll (ClangSharp also in project/References/.NET Assembly) in the same directory as the addin dll's directory. CLangSharp and user libclang.so at: 

https://github.com/gudaol/CBinding-addin/raw/roslyn/CBinding/ClangSharp.dll
https://github.com/gudaol/CBinding-addin/raw/roslyn/CBinding/libclang.so
