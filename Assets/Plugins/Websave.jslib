mergeInto(LibraryManager.library, {

  GetLocalData: function () {
      var saveData = window.localStorage.getItem('GTK_HOME_GAME_SAVE');

      if(!saveData)
      {
          saveData = "";
      }
      console.log(saveData);
        var bufferSize = lengthBytesUTF8(saveData) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(saveData, buffer, bufferSize);
        return buffer;
  },

  SetLocalData: function (saveData) {
    console.log(saveData);
    window.localStorage.setItem('GTK_HOME_GAME_SAVE', Pointer_stringify(saveData));
  },

});