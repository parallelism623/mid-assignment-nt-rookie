import axios from "axios";
import { environment } from "../constants/environment";
import { axiosAuthentication } from "./axiosAuthenticationConfig";
let notify = null;
export const setNotifier = (api) => {
  notify = api;
};
let isRefresh = false;
let queueRequestFailWhenRefreshToken = [];

const axiosClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL || environment.host,
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 10_000,
  validateStatus: (status) => status < 400,
});

axiosClient.interceptors.request.use((config) => {
  const token = JSON.parse(localStorage.getItem(environment.accessToken));
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

axiosClient.interceptors.response.use(
  (response) => {
    if (
      response.config.method === "get" &&
      response.config.url.includes("/exports/")
    ) {
      const contentType = response.headers["content-type"];

      const disposition = response.headers["content-disposition"];
      let filename = "downloaded-file";

      if (disposition && disposition.includes("filename=")) {
        const match = disposition.match(
          /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/
        );
        if (match && match[1]) {
          filename = match[1].replace(/['"]/g, "");
        }
      }

      const blob = new Blob([response.data], { type: contentType });
      const href = URL.createObjectURL(blob);

      const link = document.createElement("a");
      link.href = href;
      link.setAttribute("download", filename);
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(href);
    }
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
      const refreshToken = JSON.parse(localStorage.getItem("refresh_token"));
      if (!!error.response?.headers["token-expired"] && !isRefresh) {
        isRefresh = true;
        try {
          const accessToken = JSON.parse(localStorage.getItem("access_token"));
          const refreshResponse = await axiosAuthentication.post(
            "/auth/token-refresh",
            {
              refreshToken: refreshToken,
              accessToken: accessToken,
            }
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
          queueRequestFailWhenRefreshToken.forEach(
            ({ config, resolve, reject }) => {
              axiosClient
                .request(config)
                .then((response) => resolve(response))
                .catch((err) => reject(err));
            }
          );
          queueRequestFailWhenRefreshToken.length = 0;
          return axiosClient(originalRequest);
        } catch (refreshError) {
          localStorage.removeItem(environment.accessToken);
          localStorage.removeItem("refresh_token");

          window.location.href = "/signin";
          return Promise.reject(refreshError);
        } finally {
          isRefresh = false;
        }
      }
      if (!isRefresh) {
        localStorage.removeItem(environment.accessToken);
        localStorage.removeItem("refresh_token");

        window.location.href = "/signin";
      }
      return new Promise((resolve, reject) => {
        queueRequestFailWhenRefreshToken.push({
          config: originalRequest,
          resolve,
          reject,
        });
      });
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
