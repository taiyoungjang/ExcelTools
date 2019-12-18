#ifndef CONFIG_H
#define CONFIG_H
#ifdef __GLIBC__
# include <endian.h>
#endif

#if defined(__i386)     || defined(_M_IX86) || defined(__x86_64)  || \
    defined(_M_X64)     || defined(_M_IA64) || defined(__alpha__) || \
    defined(__ARMEL__) || defined(_M_ARM_FP) || defined(__arm64) || \
    defined(__MIPSEL__) || (defined(__BYTE_ORDER) && (__BYTE_ORDER == __LITTLE_ENDIAN))
#   define LITTLE_ENDIAN
#elif defined(__sparc) || defined(__sparc__) || defined(__hppa)      || \
      defined(__ppc__) || defined(__powerpc) || defined(_ARCH_COM) || \
      defined(__MIPSEB__) || (defined(__BYTE_ORDER) && (__BYTE_ORDER == __BIG_ENDIAN))
#   define BIG_ENDIAN
#else
#   error "Unknown architecture"
#endif
#endif //CONFIG_H
