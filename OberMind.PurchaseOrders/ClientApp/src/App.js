import React from "react";
import { Route, Routes } from "react-router-dom";
import AppRoutes from "./AppRoutes";
import { Layout } from "./components/Layout";
import {Login} from "./components/Login";
import useToken from "./hooks/userHooks";
import "./custom.css";

function App() {
  const { token, setToken } = useToken();

  if (!token) { 
    return (
      <Login setToken={setToken}/>
    )
  }
  return (
    <Layout>
      <Routes>
        {AppRoutes.map((route, index) => {
          const { element, ...rest } = route;
          return <Route key={index} {...rest} element={element} />;
        })}
      </Routes>
    </Layout>
  );
}

export default App;
