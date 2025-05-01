import axios from "axios";
import { environment } from "../constants/environment";

export const axiosAuthentication = axios.create({
  baseURL: import.meta.env.VITE_API_URL || environment.host,
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 10_000,
  validateStatus: (status) => status < 400,
});

axiosAuthentication.interceptors.request.use((config) => {
  const token = JSON.parse(localStorage.getItem(environment.accessToken));
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

axiosAuthentication.interceptors.response.use((response) => {
  return response.data?.data;
});
