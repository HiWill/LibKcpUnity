LOCAL_PATH       :=  $(call my-dir)
FILE_LIST := $(wildcard $(LOCAL_PATH)/../Assets/Plugins/iOS/LibKcp/*.cpp)
FILE_LIST += $(wildcard $(LOCAL_PATH)/../Assets/Plugins/iOS/LibKcp/*.c)

include $(CLEAR_VARS) 

LOCAL_ARM_MODE  := arm
LOCAL_PATH      := $(NDK_PROJECT_PATH)
LOCAL_MODULE    := libKcp
LOCAL_CFLAGS    := -Werror
LOCAL_SRC_FILES := $(FILE_LIST:$(LOCAL_PATH)/%=%)
LOCAL_LDLIBS    := -llog
LOCAL_CPPFLAGS := -fexceptions  
LOCAL_CPPFLAGS += -std=c++11

include $(BUILD_SHARED_LIBRARY)