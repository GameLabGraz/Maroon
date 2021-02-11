mergeInto(LibraryManager.library, {
	AntaresExecuteUserAgent: function(command) {
		if(window.antaresInterface) {
			window.antaresInterface.executeCommand(Pointer_stringify(command));
		}
	}
});