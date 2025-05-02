import axiosClient from "../config/axiosConfig";

export const roleServices = {
  get: () => axiosClient.get("/roles"),
};
