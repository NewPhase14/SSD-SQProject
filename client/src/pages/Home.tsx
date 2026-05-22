import { useEffect, useState } from "react";
import { listingClient } from "../api/clients";
import type { ListingResponseDto } from "../generated-client";

export default function Home({
                                 onOpenListing
                             }: {
    onOpenListing: (id: string) => void;
}) {
    const [listings, setListings] = useState<ListingResponseDto[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        listingClient.getAll()
            .then(setListings)
            .finally(() => setLoading(false));
    }, []);

    return (
        <div className="max-w-6xl mx-auto px-4 py-10">

            <h1 className="text-3xl font-bold text-gray-900 mb-6">
                Discover Listings
            </h1>

            {loading ? (
                <div className="text-gray-500">Loading...</div>
            ) : listings.length === 0 ? (
                <div className="text-gray-500">No listings found</div>
            ) : (
                <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-6">

                    {listings.map((l) => (
                        <div
                            key={l.id}
                            onClick={() => l.id && onOpenListing(l.id)}
                            className="bg-white border border-gray-100 rounded-2xl shadow-sm hover:shadow-lg transition cursor-pointer overflow-hidden"
                        >

                            {/* ONLY FIRST IMAGE */}
                            <div className="h-48 bg-gray-100">
                                {l.imageUrls?.[0] ? (
                                    <img
                                        src={l.imageUrls[0]}
                                        className="w-full h-full object-cover hover:scale-105 transition duration-300"
                                    />
                                ) : (
                                    <div className="h-full flex items-center justify-center text-gray-400">
                                        No image
                                    </div>
                                )}
                            </div>

                            <div className="p-4">
                                <div className="font-semibold text-gray-900 text-lg truncate">
                                    {l.title}
                                </div>

                                <div className="text-sm text-gray-500 line-clamp-2 mt-1">
                                    {l.description}
                                </div>

                                <div className="mt-4 font-bold text-green-600">
                                    {l.price} DKK
                                </div>
                            </div>

                        </div>
                    ))}

                </div>
            )}
        </div>
    );
}