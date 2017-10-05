ANDROID_NDK_ROOT=/Users/yangfan/Documents/android-sdk/NDK/android-ndk-r10e

$ANDROID_NDK_ROOT/ndk-build NDK_PROJECT_PATH=. NDK_APPLICATION_MK=Application.mk $*
mv libs/armeabi/libKcp.so ../Assets/Plugins/Android/

rm -rf libs
rm -rf obj