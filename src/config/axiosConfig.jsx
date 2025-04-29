import axios from "axios";
import { environment } from "../constants/environment";

const axiosClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL || environment.host,
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 10_000,
  validateStatus: (status) => status < 400,
});

let notify = null;
export const setNotifier = (api) => {
  notify = api;
};

axiosClient.interceptors.request.use((config) => {
  const token = JSON.parse(localStorage.getItem(environment.accessToken));
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

axiosClient.interceptors.response.use(
  (response) => {
    if (
      response.config.method !== "get" &&
      typeof response.data?.data === "string"
    ) {
      notify?.success({
        message: "Success",
        description: response.data?.data ?? "Operation successful",
        placement: "bottomRight",
      });
    }
    return response.data?.data;
  },
  async (error) => {
    if (error.response?.status === 401) {
      const originalRequest = error.config;

      if (!!error.response?.headers["token-expired"]) {
        try {
          const refreshToken = JSON.parse(
            localStorage.getItem("refresh_token")
          );
          const accessToken = JSON.parse(localStorage.getItem("access_token"));
          const refreshResponse = await axiosClient.post(
            "/auth/token-refresh",
            { refreshToken: refreshToken, accessToken: accessToken }
          );

          localStorage.setItem(
            "access_token",
            JSON.stringify(refreshResponse.accessToken) ?? ""
          );
          localStorage.setItem(
            "refresh_token",
            JSON.stringify(refreshResponse.refreshToken) ?? ""
          );

          originalRequest.headers.Authorization = `Bearer ${refreshResponse.accessToken}`;

          return axiosClient(originalRequest);
        } catch (refreshError) {
          localStorage.removeItem(environment.accessToken);
          localStorage.removeItem("refresh_token");

          window.location.href = "/signin";
          return Promise.reject(refreshError);
        }
      }
    }

    const notiError = error.response?.data?.errors?.[0]?.message;
    if (!!notiError) {
      notify?.error({
        message: `Failure`.trim(),
        placement: "bottomRight",
        description: notiError,
      });
    }

    return Promise.reject(error);
  }
);

export default axiosClient;
