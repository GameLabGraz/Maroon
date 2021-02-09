mergeInto(LibraryManager.library,
{
    GetUrlParameter: function(url_parameter_name_ptr)
    {   
        // Get requested parameter
        var url_parameter_name = Pointer_stringify(url_parameter_name_ptr);

        // Extract requested parameter from URL
        var searchParams = new URLSearchParams(window.location.search);
        var returnString = "";

        if(searchParams.has(url_parameter_name))
        {
            returnString = searchParams.get(url_parameter_name);
        }

        // Send it back to Unity
        var bufferSize = lengthBytesUTF8(returnString) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnString, buffer, bufferSize);
        return buffer;
    }
});
