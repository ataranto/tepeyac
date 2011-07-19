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
    LSSharedFileListInsertItemURL(loginItems, kLSSharedFileListItemLast, NULL, NULL, url, NULL, NULL);
	
	CFRelease(loginItems);
	CFRelease(path);
	CFRelease(url);
	if (item)
	{
		CFRelease(item);
	}
}