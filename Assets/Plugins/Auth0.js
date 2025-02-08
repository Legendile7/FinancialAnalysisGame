mergeInto(LibraryManager.library, {
    GetAuthToken: function () {
        var hash = window.location.hash;
        if (hash.includes("access_token=")) {
            var token = hash.split("access_token=")[1].split("&")[0];
            return allocate(intArrayFromString(token), ALLOC_NORMAL);
        }
        return allocate(intArrayFromString(""), ALLOC_NORMAL);
    }
});
