# KernelSU Next manager (ExpressiveOS). Android never unpacks native libs for a
# system app it hasn't updated, so the build installs the manager's libs in its
# nativeLibraryDir, and the manager runs libksud.so from there as a program.
# Files on /product are 0644 unless listed here.
[product/app/KernelSUNext/lib/arm64/libksud.so]
mode: 0755
user: AID_ROOT
group: AID_ROOT
caps: 0
