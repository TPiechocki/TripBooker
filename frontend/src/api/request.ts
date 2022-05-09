import axios, {Method} from "axios";

export const request = <D>(method: Method, url: string, data?: D, auth?: {username: string, password: string}
) => {
  return axios({
    baseURL: process.env.WEB_API_URL,
    method: method,
    url: url,
    data: data,
    auth: auth,
  }).then(data => data.data).catch(error => {
    if (error.response.status === 401) {
      throw error;
    }
    console.error(error);
  });
}
