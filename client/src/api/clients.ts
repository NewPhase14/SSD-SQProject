import {
    AuthClient,
    ListingClient,
    CategoryClient,
} from "../generated-client";

const baseUrl = "https://localhost:5001";

export const authClient = new AuthClient(baseUrl);
export const listingClient = new ListingClient(baseUrl);
export const categoryClient = new CategoryClient(baseUrl);
