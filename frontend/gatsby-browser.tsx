import React from "react"
import UserContextProvider from "./src/context/UserContext"

export const wrapRootElement = ({ element }: {element: React.ReactChild}) => (
  <UserContextProvider>{element}</UserContextProvider>
)
