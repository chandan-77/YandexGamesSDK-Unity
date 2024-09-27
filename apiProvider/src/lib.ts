mergeInto(LibraryManager.library, {
    AuthenticateUser: function() {
      if (typeof window.AuthenticateUser === 'function') {
        window.AuthenticateUser();
      } else {
        console.error('AuthenticateUser is not defined on window.');
      }
    },
  });