#include <CoreServices/CoreServices.h>

void
AddLoginExecPath(const char *cpath)
{
	LSSharedFileListRef items =
    LSSharedFileListCreate(kCFAllocatorDefault, kLSSharedFileListSessionLoginItems, NULL);
  
	if (!items)
	{
		return;
	}
	
	CFStringRef path = CFStringCreateWithCString(kCFAllocatorDefault, cpath, kCFStringEncodingUTF8);
	CFURLRef url = CFURLCreateWithFileSystemPath(kCFAllocatorDefault, path, kCFURLPOSIXPathStyle, false);	
	LSSharedFileListItemRef item =
    LSSharedFileListInsertItemURL(items, kLSSharedFileListItemLast, NULL, NULL, url, NULL, NULL);
	
	CFRelease(items);
	CFRelease(path);
	CFRelease(url);
	if (item)
	{
		CFRelease(item);
	}
}