import { useState } from "react";
import Navbar from "./components/Navbar";
import Home from "./pages/Home";
import Login from "./pages/Login";
import Register from "./pages/Register";
import CreateListing from "./pages/CreateListing";
import ListingDetails from "./pages/ListingDetails";

export default function App() {
    const [page, setPage] = useState("home");
    const [selectedListingId, setSelectedListingId] = useState<string | null>(null);

    return (
        <div className="min-h-screen bg-gray-50">
            <Navbar onNavigate={setPage} />

            {page === "home" && (
                <Home
                    onOpenListing={(id) => {
                        setSelectedListingId(id);
                        setPage("details");
                    }}
                />
            )}

            {page === "login" && <Login onNavigate={setPage} />}

            {page === "register" && <Register onNavigate={setPage} />}

            {page === "create" && <CreateListing />}

            {page === "details" && selectedListingId && (
                <ListingDetails
                    listingId={selectedListingId}
                    onBack={() => setPage("home")}
                />
            )}
        </div>
    );
}