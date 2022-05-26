mergeInto(LibraryManager.library, {

  SceneInitialized: function() {
    window.dispatchReactUnityEvent("SceneInitialized");
  }

});