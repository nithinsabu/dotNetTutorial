import React from "react";
import { it, describe, expect, vi} from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import ViewImages from "../ViewImages";
import "@testing-library/jest-dom/vitest";
import userEvent from "@testing-library/user-event";
import axios from "axios";
import * as mockRedux from "react-redux"
import store from "../redux/store"
import { Provider } from "react-redux";
vi.mock("axios");
axios.get.mockImplementation((url) => {
    if (url ===`${import.meta.env.VITE_API_URL}/api/image/`){
        return {data: [{"id":1,"name":"Cat","description":"Description 1."},{"id":2,"name":"Dog","description":"Description 2"}]
    }
    }
})
axios.delete.mockImplementation((url) => {
    return "";
})
function Wrapper({ children }) {
    return <Provider store={store}>{children}</Provider>
  }
describe("ViewImages", () => {
    render(<ViewImages/>, {wrapper: Wrapper});
    it("Renders component", () => {
        expect(screen.getByRole("heading", {name: "Images"})).toBeInTheDocument();
    })

    it("Fetched images are displayed", () => {
        const images = screen.getAllByRole("img");
        expect(images[0].src).toBe(`${import.meta.env.VITE_API_URL}/api/image/1`)
        expect(images[1].src).toBe(`${import.meta.env.VITE_API_URL}/api/image/2`)
        expect(screen.getByText(/cat/i)).toBeInTheDocument();
        expect(screen.getByText(/dog/i)).toBeInTheDocument();
    })
    it("Deletes image when button is clicked", async () => {
        const buttons = screen.getAllByRole("button", {name: "Delete"});
        await userEvent.click(buttons[0])
        expect(screen.queryByText(/cat/i)).not.toBeInTheDocument();
    })
});
