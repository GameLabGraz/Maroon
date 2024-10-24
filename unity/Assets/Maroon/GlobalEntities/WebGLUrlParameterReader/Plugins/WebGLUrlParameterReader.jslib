mergeInto(LibraryManager.library,
{
    _getUrlParameter: function(url_parameter_name_ptr)
    {   
        // Get requested parameter
        var url_parameter_name = UTF8ToString(url_parameter_name_ptr);

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
    },

    _getAllUrlParameters: function()
    {
        var searchParams = new URLSearchParams(window.location.search);
        var returnDict = {};

        for (var pair of searchParams.entries())
        {
            returnDict[pair[0]] = pair[1];
        }

        // Convert the dictionary to a JSON string
        var returnString = JSON.stringify(returnDict);

        // Send it back to Unity
        var bufferSize = lengthBytesUTF8(returnString) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnString, buffer, bufferSize);
        return buffer;
    }
});
