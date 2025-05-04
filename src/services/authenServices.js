import axiosClient from "../config/axiosConfig";

export const authenServices = {
  login: (loginModel) =>
    axiosClient.post("/auth/login", { email: "", username: "", ...loginModel }),
  register: (registerModel) =>
    axiosClient.post("/auth/register", registerModel),
  logout: () => axiosClient.post("/auth/logout"),
  emailConfirm: (requestModel) =>
    axiosClient.post("/auth/email-confirm", requestModel),
  emailConfirmRefresh: (requestModel) =>
    axiosClient.post("/auth/email-confirm-refresh", requestModel),
  changePassword: (requestModel) =>
    axiosClient.post("/auth/change-password", requestModel),
};
