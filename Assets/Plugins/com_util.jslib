mergeInto(LibraryManager.library, {
	OnDataReceived: function (_jsonStringPointer) {
		var jsonString = "";
		try {
			jsonString = Pointer_stringify(_jsonStringPointer);
			var pObject = JSON.parse(jsonString);
			if(pObject === undefined) {
				throw "Failed to parse!";
			}
			OnUnityDataReceived(pObject);
		} catch (_exception) {
			console.log("Error occurred while parsing given input string. " + jsonString);
			console.log(_exception);
		}
	},
});