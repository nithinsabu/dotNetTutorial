import React from "react";
import { it, describe, expect, vi} from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import UploadImage from "../UploadImage";
import "@testing-library/jest-dom/vitest";
import userEvent from "@testing-library/user-event";
import axios from "axios";
vi.mock("axios");
describe("UploadImage", () => {
    render(<UploadImage />);
    axios.post.mockResolvedValue({
        data: { "Person 1": "0.93", "Car 1": "0.32" },
      });
    it("renders Component", async () => {
        expect(screen.getByRole("heading", { name: "Upload Image" })).toBeInTheDocument();
        const fileInput = screen.getByLabelText(/image file/i);
        const descriptionInput = screen.getByLabelText(/description/i);
        expect(fileInput).toBeInTheDocument();
        expect(descriptionInput).toBeInTheDocument();

    });
    it ("upload works", async () => {
        const fileInput = screen.getByLabelText(/image file/i);
        fileInput.required = false;
        const descriptionInput = screen.getByLabelText(/description/i);
        const file = new File(["dummy content"], "test.png", { type: "image/png" });

        await userEvent.upload(fileInput, file);
        await userEvent.type(descriptionInput, "sample");

        await userEvent.click(screen.getByRole("button"));
        await waitFor(() => {
            const items = screen.getAllByRole("listitem");
            expect(items[0]).toHaveTextContent("Person 1: 0.93");
            expect(items[1]).toHaveTextContent("Car 1: 0.32");
        });
    })
    it("Button is disabled and re-enabled on click", async () => {
        const button = screen.getByRole("button")
        await userEvent.click(button);
        waitFor(() => {
            expect(button).toBeDisabled();
        })
        waitFor(() => {
            expect(button).not.toBeDisabled();
        })

    })
});
