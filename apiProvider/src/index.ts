import { AuthModule } from "./modules/authModule";

mergeInto(LibraryManager.library, {
  AuthenticateUser: AuthModule.AuthenticateUser,
});
