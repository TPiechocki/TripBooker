import axios, {Method} from "axios";

export const request = <D>(method: Method, url: string, data?: D
) => axios({
    baseURL: process.env.WEB_API_URL,
    method: method,
    url: url,
    data: data,
  }).then(data => data.data).catch(error => console.error(error))
