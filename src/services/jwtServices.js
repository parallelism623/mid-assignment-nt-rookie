import { jwtDecode } from "jwt-decode";

export const jwtServices = {
  decode: (token) => {
    try {
      return jwtDecode(token);
    } catch {
      return null;
    }
  },
};
